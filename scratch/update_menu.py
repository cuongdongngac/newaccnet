with open(r'D:\NewaccNet\NewaccNet.Wpf\MainWindow.xaml', 'r', encoding='utf-8') as f:
    text = f.read()

target = '</dxr:RibbonPageGroup>\r\n                </dxr:RibbonPage>'
if target not in text:
    target = '</dxr:RibbonPageGroup>\n                </dxr:RibbonPage>'

ribbon_group = '''</dxr:RibbonPageGroup>

                    <dxr:RibbonPageGroup Caption="Hóa đơn điện tử">
                        <dxb:BarButtonItem Content="Tải HĐĐT" Hint="Tải hóa đơn từ Cổng Thuế" 
                                           LargeGlyph="{dx:SvgImageSource Uri='pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/outlook%20inspired/download.svg'}" 
                                           ItemClick="BtnHddtDownload_ItemClick">
                            <dxb:BarButtonItem.SuperTip>
                                <dx:SuperTip>
                                    <dx:SuperTipHeaderItem Content="Tải Hóa Đơn Điện Tử"/>
                                    <dx:SuperTipItem Content="Kết nối Cổng Thuế (hoadondientu.gdt.gov.vn) để tải hóa đơn mua vào/bán ra lưu về máy tính."/>
                                </dx:SuperTip>
                            </dxb:BarButtonItem.SuperTip>
                        </dxb:BarButtonItem>
                        <dxb:BarButtonItem Content="Xem &amp; Nhập HĐ" Hint="Xem trước hóa đơn và nhập vào CSDL" 
                                           LargeGlyph="{dx:SvgImageSource Uri='pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_invoice.svg'}" 
                                           ItemClick="BtnHddtReview_ItemClick">
                            <dxb:BarButtonItem.SuperTip>
                                <dx:SuperTip>
                                    <dx:SuperTipHeaderItem Content="Xem Trước &amp; Nhập HĐĐT"/>
                                    <dx:SuperTipItem Content="Xem trước các hóa đơn từ tệp JSON, kiểm tra chi tiết thuế suất/mặt hàng và nạp vào CSDL làm việc."/>
                                </dx:SuperTip>
                            </dxb:BarButtonItem.SuperTip>
                        </dxb:BarButtonItem>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>'''

if target in text:
    text = text.replace(target, ribbon_group, 1)
    with open(r'D:\NewaccNet\NewaccNet.Wpf\MainWindow.xaml', 'w', encoding='utf-8') as f:
        f.write(text)
    print('Updated MainWindow.xaml successfully!')
else:
    print('Target not found!')
