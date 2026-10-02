using System;
using System.Windows;

namespace NewaccNet.Wpf.AppSystem
{
    public static class AuthHelper
    {
        /// <summary>
        /// Biến tĩnh lưu trữ RoleMask của người dùng đang đăng nhập.
        /// Hãy gán giá trị này ngay sau khi người dùng đăng nhập thành công.
        /// </summary>
        public static int CurrentUserMask { get; set; } = 0;

        // Định nghĩa Attached Property "RequiredMask"
        public static readonly DependencyProperty RequiredMaskProperty =
            DependencyProperty.RegisterAttached(
                "RequiredMask", 
                typeof(int), 
                typeof(AuthHelper), 
                new PropertyMetadata(0, OnRequiredMaskChanged));

        public static int GetRequiredMask(DependencyObject obj)
        {
            return (int)obj.GetValue(RequiredMaskProperty);
        }

        public static void SetRequiredMask(DependencyObject obj, int value)
        {
            obj.SetValue(RequiredMaskProperty, value);
        }

        private static void OnRequiredMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int requiredMask = (int)e.NewValue;
            bool hasPermission = requiredMask == 0 || (requiredMask & CurrentUserMask) > 0;

            if (d is UIElement uiElement)
            {
                uiElement.IsEnabled = hasPermission;
            }
            else if (d is DevExpress.Xpf.Bars.BarItem barItem)
            {
                barItem.IsEnabled = hasPermission;
            }
            else if (d is FrameworkContentElement fce)
            {
                fce.IsEnabled = hasPermission;
            }
        }
    }
}
