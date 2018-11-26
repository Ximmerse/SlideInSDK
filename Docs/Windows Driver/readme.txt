4. Windows平台安装libusb库（AGIA设备的驱动程序）
1)、将我们的UVC设备插入PC，正常可以看到视频通信设备（VID 0x1BF3  PID 0x1010）
2)、管理器权限下执行  Tools/libusb-win32-bin-1.2.6.0下的inf-wizard.exe，按照提示往下走即可；
3)、当程序提示需要选择USB接口是，选择（VID 0x1BF3  PID 0x1010）的设备就可以了；
4)、安装成功在设备管理器中能够看到 libusb(WinUSB) devices  下面有一个 AR UVC