# 入门  :id=docs

> Slide-In AR Headset是一款出色、快速响应的AR头戴设备。

本章节将会带领开发者建立对SDK的基本了解以及普及基本概念。



## Slide-In AR Headset

Slide-In AR Headset（以下简称Slide-In） 是一款由XIMMERSE 研发的AR头戴设备，采用自研的光学结构以及跟踪模组。

核心部件由VPU（视觉处理单元）、指定的手机设备（处理数据以及提供图像显示）以及双眼反射透镜（反射并呈现图像内容）所组成。

Slide-In 的VPU采用高速IR摄像头，可以快速捕捉Marker 的姿态数据，传至手机设备进行处理与叠加视觉效果，并且进行镜片反畸变处理后显示到手机屏幕上以实现用户空间定位、动作捕捉以及识别用户姿态的功能。

以现实世界中用户的真实感官与全息影像叠加，实现丰富的感官体验与灵活的交互控制的目的。

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/slide-in-ar-headset.png)



## Slide-In 软件开发套件

Slide-In SDK Github地址：[https://github.com/Ximmerse/SlideInSDK](https://github.com/Ximmerse/SlideInSDK)

Slide-In SDK 是专门为Slide-In 头戴设备设计使用的软件开发套件。

其中主要内容包含了开发用到的核心套件（底层Tracking Module的驱动以及封装）以及辅助开发的效率工具。

由于Marker能根据不同的数量、不同的姿态形成新的组合，而达到不同的目的，我们需要在Slide-In SDK 中针对硬件情况的配置选项。



##  Marker 的硬件类型

!> 每种Marker都会有自己特定的Id标记，上面会有Marker的Id信息以及Marker的类型信息。

### 单个Marker

可独立使用的最小的可识别单位。每个Marker外观约为38mm * 38mm大小的正方形。

### 组合Marker

组合Marker是有多个指定Id的Marker按照固定姿态排布所形成的一个整体识别目标。目前分为以下几种样式：

#### 桌布 (Map)

Map是由指定Id的16个单个Marker按一定规律平铺组成的一个整体识别目标。每个Map的外观约为380mm * 380mm大小的正方形。

Map的特点如下：

- 更远追踪范围
- 具有一定的抗遮挡能力
- 可视角度与普通单Marker一致

#### 魔方 (Cube)

  ![Cube](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/cube-1.png)

  Cube是由指定Id的18个单Marker按照一定姿态组成的一个整体识别目标。

  Cube的特点如下：

  - 稍远的追踪范围

  - 全角度可视

  - 范围内追踪稳定

#### 枪型 (Gun Handle)

  ![Gun Handle](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/gun-handle.png)

  ![Gun Handle 2](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/gun-handle-2.png)

  Gun是由指定Id的4个单Marker按照一定姿态组成的一个整体识别目标。

  Gun的特点如下：

  - 相对较大的可视角度
  - 占用比较少的Marker ID数量
