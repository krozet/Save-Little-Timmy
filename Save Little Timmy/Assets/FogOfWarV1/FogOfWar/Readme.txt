FOG OF WAR GPU
This is a GPU based fog of war solution. It can efficiently process a large number of units of data computing, and even has very good performance on mobile devices.
 It is designed according to the war fog function in the League of Legends, so it has almost all the functions of the war fog in the League of Legends.


Current limitations:
In addition to the use of custom obstacle maps (which have higher requirements for art work), they are currently only supported on relatively flat terrain and environments.
Shader model 3.5 and above required

Main functions:
Multi level adjustable fog accuracy (fog map resolution), update rate and supersampling enable or not.
Fully customizable fog color, transparency, smooth transition speed, fog update rate.
Two Fog Area update types: Bake (render at first frame) / Every Frame.
Custom obstacle map and procedure obstacle map export.
Automatically copies the fog map to rendertexture (for UI display if set).
Built in mini map rendering tool.
Two kinds of built-in fog materials for high or low performance devices respectively.
Support dynamic obstacles.
Support multiple vision units, maskable units and dynamic occluders.
Support fog clearing in specified area (circle only).
Simple interface and easy to secondary development.

Additional features:
Simple Joysticks(base on UGUI).
Simple blured UI features.
Special occlusion dissolving shader.


Quick Start:
1. Setup the scene, and set the layer for the obstacles(such as the wall).
2. Create empty GameObject &amp; add "FogArea" component.
3. Set "obstancleLayer" to the layer in 1 in FogArea component.
4. Click the "generate obstacle map" button to preview the obstacle map.
5. Click the "create renderer" button to create a default fog renderer (high quality render).
6. Add IVision object and IMaskable object to the scene.
7. Any Questions : jackluoli6@gmail.com


How it works: 
1.Render obstacle map
2.Render  dynamic occluders map
3.Render vision map
4.Up-scale and blurry
5.Smooth fade transition
