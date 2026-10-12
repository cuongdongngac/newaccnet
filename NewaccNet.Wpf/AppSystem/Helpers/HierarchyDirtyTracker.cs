using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Helpers
{
    /// <summary>
    /// Trigger/Tracker tổng hợp giám sát cây dữ liệu phân cấp (Hierarchy / Domino Pattern).
    /// Hoàn toàn độc lập với UI/Grid, chỉ hoạt động trên các Entity và EntityCollection của LLBLGen.
    /// Khi bất kỳ Entity con/cháu nào ở tầng sâu bị thay đổi thuộc tính, thêm mới dòng hoặc xóa dòng,
    /// tracker sẽ tự động kích hoạt hiệu ứng Domino đánh dấu Entity cha trực tiếp là Dirty (IsDirty = true),
    /// và tiếp tục lan truyền lên các tầng cha bên trên cho đến tầng Root.
    /// </summary>
    public class HierarchyDirtyTracker : IDisposable
    {
        private class ChildNavigator
        {
            public Type ChildType { get; set; } = null!;
            public Func<EntityBase2, IEntityCollection2?> GetCollection { get; set; } = null!;
        }

        // Ánh xạ Type của Entity cha -> Danh sách các navigator lấy collection con
        private readonly Dictionary<Type, List<ChildNavigator>> _navigators = new Dictionary<Type, List<ChildNavigator>>();

        // Ánh xạ Entity con -> Entity cha trực tiếp
        private readonly Dictionary<EntityBase2, EntityBase2> _parentMap = new Dictionary<EntityBase2, EntityBase2>();

        // Ánh xạ Collection con -> Entity cha sở hữu collection đó
        private readonly Dictionary<IEntityCollection2, EntityBase2> _collectionParentMap = new Dictionary<IEntityCollection2, EntityBase2>();

        // Tập hợp các Entity đang được lắng nghe
        private readonly HashSet<EntityBase2> _trackedEntities = new HashSet<EntityBase2>();

        // Tập hợp các Collection đang được lắng nghe
        private readonly HashSet<IEntityCollection2> _trackedCollections = new HashSet<IEntityCollection2>();

        // Root Collection
        private IEntityCollection2? _rootCollection;

        /// <summary>
        /// Sự kiện được bắn ra mỗi khi có một Entity (ở bất kỳ tầng nào) bị đánh dấu Dirty
        /// </summary>
        public event Action<EntityBase2>? EntityDirtied;

        /// <summary>
        /// Đăng ký quan hệ 1-N giữa Entity cha và Collection con.
        /// </summary>
        /// <typeparam name="TParent">Kiểu Entity cha</typeparam>
        /// <typeparam name="TChild">Kiểu Entity con</typeparam>
        /// <param name="childCollectionSelector">Hàm lấy Collection con từ cha</param>
        public HierarchyDirtyTracker RegisterChild<TParent, TChild>(Func<TParent, EntityCollection<TChild>?> childCollectionSelector)
            where TParent : EntityBase2
            where TChild : EntityBase2
        {
            var parentType = typeof(TParent);
            if (!_navigators.TryGetValue(parentType, out var list))
            {
                list = new List<ChildNavigator>();
                _navigators[parentType] = list;
            }

            list.Add(new ChildNavigator
            {
                ChildType = typeof(TChild),
                GetCollection = p => p is TParent tp ? childCollectionSelector(tp) : null
            });

            return this;
        }

        /// <summary>
        /// Gắn Tracker vào Root Collection và toàn bộ cây phân cấp bên dưới.
        /// Tự động hủy các liên kết cũ nếu đã gắn trước đó.
        /// </summary>
        public void Attach(IEntityCollection2 rootCollection)
        {
            DetachAll();

            if (rootCollection == null) return;
            _rootCollection = rootCollection;

            HookCollection(rootCollection, parent: null);

            foreach (var item in rootCollection)
            {
                if (item is EntityBase2 entity)
                {
                    TrackEntity(entity, parent: null);
                }
            }
        }

        /// <summary>
        /// Đánh dấu một Entity là Dirty và kích hoạt Domino Bubble-Up lên toàn bộ các tầng cha.
        /// </summary>
        public void MarkDirtyAndBubbleUp(EntityBase2? entity)
        {
            if (entity == null) return;

            entity.Fields.IsDirty = true;
            EntityDirtied?.Invoke(entity);

            if (_parentMap.TryGetValue(entity, out var parent) && parent != null)
            {
                MarkDirtyAndBubbleUp(parent);
            }
        }

        private void TrackEntity(EntityBase2 entity, EntityBase2? parent)
        {
            if (entity == null || _trackedEntities.Contains(entity)) return;

            _trackedEntities.Add(entity);

            if (parent != null)
            {
                _parentMap[entity] = parent;
            }

            entity.PropertyChanged += OnEntityPropertyChanged;

            // Kiểm tra xem entity này có các collection con nào đã đăng ký không
            var entityType = entity.GetType();
            foreach (var kvp in _navigators)
            {
                if (kvp.Key.IsAssignableFrom(entityType))
                {
                    foreach (var nav in kvp.Value)
                    {
                        var childCol = nav.GetCollection(entity);
                        if (childCol != null)
                        {
                            HookCollection(childCol, parent: entity);

                            foreach (var childItem in childCol)
                            {
                                if (childItem is EntityBase2 childEntity)
                                {
                                    TrackEntity(childEntity, parent: entity);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UntrackEntity(EntityBase2? entity)
        {
            if (entity == null || !_trackedEntities.Contains(entity)) return;

            entity.PropertyChanged -= OnEntityPropertyChanged;
            _trackedEntities.Remove(entity);
            _parentMap.Remove(entity);

            var entityType = entity.GetType();
            foreach (var kvp in _navigators)
            {
                if (kvp.Key.IsAssignableFrom(entityType))
                {
                    foreach (var nav in kvp.Value)
                    {
                        var childCol = nav.GetCollection(entity);
                        if (childCol != null)
                        {
                            UnhookCollection(childCol);

                            foreach (var childItem in childCol)
                            {
                                if (childItem is EntityBase2 childEntity)
                                {
                                    UntrackEntity(childEntity);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HookCollection(IEntityCollection2 collection, EntityBase2? parent)
        {
            if (_trackedCollections.Contains(collection)) return;

            _trackedCollections.Add(collection);

            if (parent != null)
            {
                _collectionParentMap[collection] = parent;
            }

            collection.EntityAdded += OnCollectionEntityAdded;
            collection.EntityRemoved += OnCollectionEntityRemoved;
        }

        private void UnhookCollection(IEntityCollection2 collection)
        {
            if (!_trackedCollections.Contains(collection)) return;

            collection.EntityAdded -= OnCollectionEntityAdded;
            collection.EntityRemoved -= OnCollectionEntityRemoved;

            _trackedCollections.Remove(collection);
            _collectionParentMap.Remove(collection);
        }

        private void OnEntityPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var entity = sender as EntityBase2;
            if (entity == null) return;

            // Bỏ qua các sự kiện đổi IsDirty/IsNew để tránh trigger thừa
            if (e.PropertyName == nameof(EntityBase2.IsDirty) || e.PropertyName == nameof(EntityBase2.IsNew))
            {
                return;
            }

            // Entity hiện tại đã có thay đổi -> Domino lên cha
            MarkDirtyAndBubbleUp(entity);
        }

        private void OnCollectionEntityAdded(object? sender, CollectionChangedEventArgs e)
        {
            var newChild = e.InvolvedEntity as EntityBase2;
            var collection = sender as IEntityCollection2;
            if (newChild == null || collection == null) return;

            if (_collectionParentMap.TryGetValue(collection, out var parent))
            {
                TrackEntity(newChild, parent);
                // Thêm con mới -> đánh dấu cha là dirty và domino lên trên
                MarkDirtyAndBubbleUp(parent);
            }
            else
            {
                // Thêm ở root collection
                TrackEntity(newChild, parent: null);
                newChild.Fields.IsDirty = true;
                EntityDirtied?.Invoke(newChild);
            }
        }

        private void OnCollectionEntityRemoved(object? sender, CollectionChangedEventArgs e)
        {
            var removedChild = e.InvolvedEntity as EntityBase2;
            var collection = sender as IEntityCollection2;
            if (collection == null) return;

            UntrackEntity(removedChild);

            if (_collectionParentMap.TryGetValue(collection, out var parent))
            {
                // Xóa con khỏi cha -> đánh dấu cha là dirty và domino lên trên
                MarkDirtyAndBubbleUp(parent);
            }
        }

        /// <summary>
        /// Gỡ bỏ toàn bộ sự kiện lắng nghe trên tất cả Entity và Collection trong cây.
        /// Giúp ngăn ngừa triệt để hiện tượng rò rỉ bộ nhớ (Memory Leak).
        /// </summary>
        public void DetachAll()
        {
            foreach (var entity in _trackedEntities)
            {
                entity.PropertyChanged -= OnEntityPropertyChanged;
            }
            _trackedEntities.Clear();

            foreach (var col in _trackedCollections)
            {
                col.EntityAdded -= OnCollectionEntityAdded;
                col.EntityRemoved -= OnCollectionEntityRemoved;
            }
            _trackedCollections.Clear();

            _parentMap.Clear();
            _collectionParentMap.Clear();
            _rootCollection = null;
        }

        public void Dispose()
        {
            DetachAll();
        }
    }
}
