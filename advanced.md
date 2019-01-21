# 进阶指南

本章节将会在快速开始的基础上，进一步的解释详细的概念以及SDK的进阶用途。



## 概念与组件

这里我们将会介绍一下实际开发中的会遇到的一些组件以及其用法。

### AR摄像机

AR摄像机是作用于在Unity场景中模拟人眼看到的信息。为了Marker的追踪以及视觉呈现的效果，场景中必须有一个附有Camera组件的GameObject，同时该GameObject上需要附有`ARCamera`以及`Tag Tracker`两个组件。可以以Demo场景中的Camera设置作为参考。

`AR Camera`组件主要用作于配置代表头显的的渲染显示以及旋转姿态。

- `Head Mode`用于配置头部旋转模式。`static`代表摄像机不更新旋转信息；`RotateLocally`代表摄像机每帧会根据IMU更新旋转头部信息。默认值是`RotateLocally`。
- `Rendering Mode`用于配置摄像机的渲染显示模式。`Stereo`代表摄像机的渲染会分成两个摄像机渲染成左右两个屏幕的画面。`Single`代表不分屏渲染，只在当前的Camera的位置进行渲染。
- `EyeCovergenceMode`用于配置在`Stereo`模式下，两个摄像机相交位置。`Infinity`代表两个摄像机的方向是平行的。`Cross Plane`代表两个摄像机会在前方向一定距离处相交。

`Tag Tracker`组件主要用于配置该Camera的Tracking Profiles，指明该AR 摄像机能识别追踪的Marker信息。

- `Tracking Profiles`在[上文](README.md)中我们提到了头戴设备能追踪一个单Marker或多个Marker的组合追踪目标，根据不同的应用场景开发者会使用到不同的硬件，Tracking Profiles就是用于分辨不同硬件追踪目标的数据文件，**在一个Profile中不能包含有相同ID 的Marker**。

### Marker Target

Marker Target 是硬件Marker在Unity虚拟场景中的载体容器。

在场景中新建一个空白的GameObject，为其添加`Marker Identity`以及`Dynamic Marker`或者`Bench Marker`（二选一），就完成了Marker Target的创建了。

- `Marker Identity`用于管理Marker Target所对应的Marker/Marker Group ID以及本身的能见度。

不管Marker类型是`Bench Marker`亦或是`Dynamic Marker` ，他们都可以使用scatter cards (单Marker)或者marker group (组合Marker) 的marker类型。

- `Bench Marker` 使用指定的Marker在Unity虚拟场景中建立Tracking的相对坐标系，`Bench Marker`所在的位置即为相对坐标系的原点（推荐使用零点）。

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

- `Dynamic Marker` 是用于追踪指定的Marker并获取其相对的姿态信息。

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

## 从头开始

如果需要从头开始搭建一个真实开发场景，开发者可以遵循以下步骤：

1. 根据应用场景选择开所需的硬件设备，再这个示例中我们选择使用Cube

   ![Cube](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/cube-1.png)

   观察硬件的id 标签。

   !> 每个marker 硬件都会对应唯一个id

   ​![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/cube-id-sample.png)



2. 打开Unity新建工程，按照[上文](quickstart.md)中的指南导入SDK并且完成项目设置。



3. 制作Tracking Profiles

   在project 栏中任意目录下点击右键，在弹出的上下文窗口中选择`Create/Ximmerse/SlideInSDK/Tracking Profile` 新建一个Tracking Profile。

   ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-create-tracking-profile.png)

   你可以自定义`Tracking Profile` 文件的名字以及使用描述。在此示例中我们命名Demo Tracking Profile。

   ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-tracking-profile-description-inspector.png)

   接下来我们添加标定文件到Tracking Profile 中。由于我们所使用的硬件是Cube，而Cube是一个组合型追踪目标，所以我们的标定文件是添加Marker Group 类型。Config 一栏我们则使用与硬件id一致 为cube-410-01-007 的json配置文件。

   ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-tracking-profile-json-inspector.png)

   至此，我们就完成Tracking Profile 的创建。

   

4. 新建场景，在场景中搭设所需要的组件。

   - AR Camera

     在Main Camera上增加`AR Camera` 以及`Tag Tracker` 两个组件。Tag Tracker 的Tracking Profile 使用我们刚刚创建的Profile。

   ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-arcamera-tag-tracker-inspector.png)

   

   - Dynamic Marker

     建立完AR Camera 后，我们需要添加Marker Target 以确保Marker 能追踪并且使用其位置信息。

     创建一个GameObject，将其命名为Cube Dynamic Marker Target。为其添加`Marker Identity` 和`Dynamic Marker` 两个组件。

     ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-cube-dynamic-marker-inspector.png)

     此时我们需要查找Cube 所对应的Marker Id， 然后将其填入`Marker Identity` 对应的字段中。

     我们可以从配置的Tracking Profile 中的`Json Config` 中找到。

     ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-cube-tracking-json-marker-id.png)

     所以我们将Marker id，这里是`GroupID` **80** 填入`Marker Identity` 的`Marker Id` 字段中。

     至此基础的Marker追踪以及摄像机渲染设置完成。

     

5. 根据应用需求，构建虚拟视图。

   在这个实例当中，我们会建立一个简单的球型视图作为虚拟视图。但用户透过头显观察Cube 的时候，会看到一个红色的球体取替了Cube的位置。

   首先我们在Marker Target 下建立新的空白子物体，命名为View Root，由于Cube 的物理尺寸在半径0.11m左右， 我们将View Root 的缩放调整成0.11。

   ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-view-root-inspector.png)

   然后我们在View Root 下建立新的Sphere 子物体，新建一个红色材质球赋予给这个sphere即可。

   ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-sphere-view-inspector.png)

   至此，虚拟视图的设置完成。

   

6. 添加测试、调试工具。（可选）

   为了能更加方便的调试，我们建议加入一下工具方便调试。

   - PE Network Sync Transform / Properties

     请参考[此处](notdoneyet.md)查阅详细用法

     

   - [LunarConsole](https://assetstore.unity.com/packages/tools/gui/lunar-mobile-console-free-82881)

     在Project 一栏搜索Lunar Console 这个Prefab，并且将其置入场景中。

     ![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-lunar-console.png)

   

7. 打包测试。

   将这个场景保存到资源中，并在Build Settings中添加此场景。

   按照流程打包成sdk即可进行测试。



## 自定义Build Preset

Build Preset 是PE Plugins 中的Build Manager 所使用的快速设置Build Settings 配置文件。

我们开始新建一个自定义的Build Preset，首先进入`Tools/PolyEngine/Build Manager` 打开Build Manager的窗界面。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-new-preset-build-manager-window.png)



点击New Preset 按钮并且为新的Preset 输入一个名称，在这里我们设置为Demo Preset。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/sample-new-preset-button.png)

选择并加载我们建立的Preset，这个时候我们可以设置对应Player Settings 中的内容，这里我们举例几个常用项进行说明：

- Build Name

  最终apk安装后显示的应用名称。

- Package ID

  apk的唯一id，一般是com.开头。

- Default Orientation

  设备的默认旋转方向。

  !> 在Slide-In 的应用场景中中，我们的必须直接选择**Landscape Right**这个选项。

- Scenes

  对应在Build Settings 中会打包进apk 的场景。

设置完成Preset 后，我们需要点击Save Preset 按钮进行保存。

至此，自定义Build Preset的设置全部完成，之后我们就可以快速切换打包设置了。

