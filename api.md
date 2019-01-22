# 接口参考说明

本章节主要以程序开发的角度阐述说明SDK 中脚本、组件的功能。



## 常规组件

介绍在SDK 场景中会出现并且使用的组件。



### ARCamera

`AR Camera`组件主要用作于配置代表头显的的渲染显示以及旋转姿态。

- `Head Mode`用于配置头部旋转模式。`static`代表摄像机不更新旋转信息；`RotateLocally`代表摄像机每帧会根据IMU更新旋转头部信息。默认值是`RotateLocally`。
- `Rendering Mode`用于配置摄像机的渲染显示模式。`Stereo`代表摄像机的渲染会分成两个摄像机渲染成左右两个屏幕的画面。`Single`代表不分屏渲染，只在当前的Camera的位置进行渲染。
- `EyeCovergenceMode`用于配置在`Stereo`模式下，两个摄像机相交位置。`Infinity`代表两个摄像机的方向是平行的。`Cross Plane`代表两个摄像机会在前方向一定距离处相交。



### Tag Tracker

`Tag Tracker`组件主要用于配置该Camera的Tracking Profiles，指明该AR 摄像机能识别追踪的Marker信息。

- `Tracking Profiles`在[上文](README.md)中我们提到了头戴设备能追踪一个单Marker或多个Marker的组合追踪目标，根据不同的应用场景开发者会使用到不同的硬件，Tracking Profiles就是用于分辨不同硬件追踪目标的数据文件，**在一个Profile中不能包含有相同ID 的Marker**。



### Marker Identity

`Marker Identity`用于管理Marker Target所对应的Marker/Marker Group ID以及本身的能见度。

`Marker ID ` 这个这段可以填写单Marker 的ID 字段，或是组合Marker的 Marker Group ID 字段。



### Bench Marker

`Bench Marker` 所建立相对坐标系是基于与AR摄像机的相对姿态变换而来的，因此`Bench Marker`本身的姿态会对这个坐标系产生影响。

我们以`Bench Marker Match Scale Content`这个Demo场景为例子说应一下实际使用情况：

该场景中使用的是物理尺寸0.38m * 0.38m的桌布(Map)组合marker，marker id是34。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/bechmarker-sample-profile.png)

首先在场景中搭建基础的AR摄像头（参考上一小节）以及`Bench Marker`。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-inspector.png)

此时，已经建立了Tracking坐标系，同时坐标系原点也在`Bench Marker`所在的(0,0,0)点。

此时我们创建一个Quad作为`Bench Marker`的硬件指示物，调整x轴旋转为90度，同时赋予它的scale为 0.38x0.38x0.38，并且使其成为`Bench Marker`的子对象。

> Quad 为比例为1m的物体，缩放尺寸为 0.38m (硬件实际尺寸) / 1m (Quad实际尺寸) = 0.38m即可。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-quad-inspector.png)

此时我们将准备好的素材放置到场景中，这是一个约为10m*10m的房子，可以看到与实际Marker相比是非常大的。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-house.png)

我们的目标是希望这个房子能在扫描追踪的时候，比例正确的贴合在marker上，所以我们要将tracking 坐标系进行缩放。而因为有`Bench Marker`的存在，我们缩放的时候直接调整`Bench Marker`的大小就可以了。

因为我们之前制作了一个Marker物理尺寸的映射Quad在虚拟场景中，它可以给我们一个很好的缩放参考。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-house-scale.png)

因此，我们将`Bench Marker` 物体整体放大，直至映射物理尺寸的Quad指示物刚好能覆盖整个房子为止，接着再细微调节房子的位置，使其完全贴合。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-house-ar.png)

至此，我们完成了以`Bench Marker` 为基础的Trakcing坐标系的缩放调整。



### Dynamic Marker

Dynamic Marker` 是用于追踪指定的Marker并获取其相对的姿态信息。

不同于`Bench Marker` 是处于建立的坐标系的原点，自身的姿态不会发生改变，`Dynamic Marker` 本身的Marker Target会每帧根据追踪的Marker更新自己的姿态信息。

因此如果我们如果需要虚拟视图根据Marker的姿态同步变化，就可以直接将Marker的虚拟视图直接放置到Marker Target下座位Marker Target的子物体。

!> 需要注意的是，`Dynamic Marker` 的Marker Target 缩放属性应始终保持1x1x1，如果需要调整虚拟视图与硬件的比例情况，应直接对虚拟视图进行缩放。

下面我们使用`Dynamic Marker _ Single Cards` 这个场景来进行举例说明：

首先我们在Main Camera 的`Tag Tracker` 上能看到我们当前使用的Tracking Profile是多个单marker的组合

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/dynamic-sample-cards-profiles.png)

选择其中任意一个Marker Target，看到当前marker 的id 是 0。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/dynamic-sample-cards-inspector.png)

​	查看硬件id 为420-001的marker卡面，测量其物理尺寸可得0.04m * 0.04m。

​	建立一个Quad作为Marker 的虚拟视图，将其放置于Dynamic Marker Target 层级之下，成为Marker Target的子物体。

​	为了贴合物理尺寸与虚拟视图尺寸，我们需要对虚拟视图进行缩放。

> Quad 为比例为1m的物体，缩放尺寸为 0.04m (硬件实际尺寸) / 1m (Quad实际尺寸) = 0.04m即可。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/dynamic-sample-marker-scale.png)

至此，Dynamic Marker的缩放已全部完成。

