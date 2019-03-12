# Introduction  :id=docs


In this article, we will introduce SDK structure and a few concepts about Slide-In AR system.



## Slide-In AR Headset

Slide-In AR Headset（aka Slide-In) is developed by XIMMERSE with vision tracking and lens technology,

The core components are VPU(Vision Processing Unit), phone (used for rendering and content processing) and reflective lens (which shows the contents to user).

Slide-In VPU unilizes a high speed IR camera, which captures Marker position and rotation data. Then, it sends the data to the phone and the phone will render the contents with inputs from VPU. The phone screen is reflected by the lens and the lense will present the final visuals to the user.

With the Slide-In, users are able to interact with real physical objects with virtual overlays on top of the physical objects, enabling a rich interactive experience.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/slide-in-ar-headset.png)



## Slide-In SDK

Slide-In SDK Github address：[https://github.com/Ximmerse/SlideInSDK](https://github.com/Ximmerse/SlideInSDK)

Slide-In SDK is designed for Slide-In AR Headset only.

The SDK contains the APIs, hardware drivers and development tools.

Because markers can be combined in a number of ways for different purpose, we would need to config markers settings in the SDK for a specific setup.



##  Marker/Device Type

!> Each Marker has its own identifier.

### Single Marker

Each Marker is around 38mm * 38mm in size.

### Marker Group

Marker group can have muitple single markers to achieve different purpose and it can be recognized by Slide-In as a single object. For example:

#### Map

Map has 16 ID'ed single markers. Every map is around 380mm * 380mm in size.

Map characteristics：

- Further tracking distance
- Can still be trackable if partially blocked
- Same view angles as the single markers

#### Cube

  ![Cube](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/cube-1.png)

  Cube has a total of 18 ID'ed markers.
  Cube characteristics：

  - slightly further tracking distance

  - 360 degree trackable

  - stable and consistent vision tracking

#### Gun Handle

  ![Gun Handle](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/gun-handle.png)

  ![Gun Handle 2](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/gun-handle-2.png)

  Gun has a total of 4 ID'ed markers. 

  Gun handle characteristics：

  - relatively larger trackable angles
  - requires small amount of ID'ed markers
