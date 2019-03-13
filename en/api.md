# API Documentation



## Components



### ARCamera

`AR Camera` is a virtual representation of the AR headset. It takes care of rendering and communicates the orientation of the physical headset to the SDK.

- `Head Mode` is used for head rotation data preference. `static` means no head rotation；`RotateLocally` means the ARCamera will update the virtual representation every frame with the IMU data. Defaule value is `RotateLocally`。
- `Rendering Mode` is used for rendering configuration. `Stereo` means stereoscoptic rendering. `Single` stands for regular 2D screen rendering.
- `EyeCovergenceMode` is used to setup up stereo rendering convergence position. `Infinity` means that 2 camera's forward are parallel to each other. `Cross Plane` means that the camera forward will cross in front of the cameras.



### Tag Tracker

`Tag Tracker` component is used to config Tracking Profiles under camera component so that the camera is able to track marker position data.

- `Tracking Profiles`, as mentioned [here](README.md), can be used to track multiple markers or multiple marker groups. Developers can config the hardware based application needs. **Please note that there can't be duplicated marker ID in a single Tracking Profile**.



### Marker Identity

`Marker Identity` is used for managing Marker Target and its Marker/Marker Group ID visibility.

`Marker ID` needs to be filled with Marker ID or Marker Group ID which can be found in JSON file. 


### Bench Marker

`Bench Marker` can be used as a world origin.

`Bench Marker` is used as a world origin, so it's placement will affect the entire coordination system in the application.

We will use `Bench Marker Match Scale Content` demo scene to explain its usage:

The marker used in this scene has a physical dimension of 0.38 meter * 0.38 meter. The marker ID is 34.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/bechmarker-sample-profile.png)

First of all, let's setup ARCamera (please refer previous tutorial for details) and `Bench Marker`.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-inspector.png)

Now, we have a tracking coordination system. Please also make sure `Bench Marker`'s position is set to (0,0,0).

We will be using a Quad object to represent the `Bench Marker`. Adjust the rotation x to 90 degrees, and then set the scale to (0.38,0.38,0.38). The last step is to drag this Quad object to `Bench Marker` as a child object.

> By Default, Unity Quad is a 1 meter x 1 meter object. Therefore we set the scale to 0.38 to match the physical object.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-quad-inspector.png)

Now, let's drag the house asset to the scene. The house asset is around 10 meter x 10 meter, so it will look much larger compared to the marker. 

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-house.png)

Our goal is the show the house on top of the real physical marker when user wears the AR headset. Because we already have `Bench Marker` as the parent object，we only have to adjust the scale of `Bench Marker`.

Because we have made a virtual representation of the physical marker, we could use it as a scale refference. 

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-house-scale.png)

Therefore, we will scale up the `Bench Marker` until it covers the entire house.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/benchmarker-sample-house-ar.png)

Now, we have finished the tracking system setup with `Bench Marker`.



### Dynamic Marker

`Dynamic Marker` is used to capture physical marker orientation and position.

Comparing to `Bench Marker`, Dynamic doesn't change the world coordination system, but it will change its own Marker Target transforms.

!> Please note that `Dynamic Marker`'s Marker Target's scale is always (1,1,1). If scale change is needed, please only adjust the children object.

We will use `Dynamic Marker _ Single Cards` as an example:

First of all, let's take a look at the the Tracking Profile.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/dynamic-sample-cards-profiles.png)

Choose any Marker Target，and set ID to 0.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/dynamic-sample-cards-inspector.png)

​	Check the hardware with ID 420-001, and measure its size, which is 0.04 meter * 0.04 meter.

​	Create a Quad as the physical marker representation, and place it under Dynamic Marker Target as a child. 

​	To closely match the virtual image to physical marker, we will need to adjust the virtual object scale.

> By Default, Unity Quad is a 1 meter x 1 meter object. Therefore we set the scale to 0.38 to match the physical object.

![](https://ximmerse-1253940012.cos.ap-guangzhou.myqcloud.com/slide-in-sdk/dynamic-sample-marker-scale.png)

Now, we have finished setting up Dynamic Marker.

