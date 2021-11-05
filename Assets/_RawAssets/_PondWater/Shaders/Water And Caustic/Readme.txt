Readme - Water and Caustic Shader
_________________________


Shaders Included : 

- Water Fast 
- Water River
- Water Ocean
(+Real Reflection)

- Caustic SpecularGloss Only (No textures, only caustic)
- Caustic SpecularGloss Fast 
- Caustic SpecularGloss Advanced



Important : Make sure to uncheck Visualize Horizon and Visualize turbulence if not needed.


Water Ocean :
___________


     
     - Main Properties -

Control the general/edge colors and the specularity.


     - Reflection Properties - 

Like most of our shaders we choose to keep the cubemap reflection to quickly enhance the overall reflection quality.
For the real reflection shaders you will need to enable "Invert Real Reflection" if you are using deferred rendering path (Don't forget to add the MirrorReflection script to the model).
(In forward rendering Path the reflection can only be seen in the game view.... "Invert Real Reflection" need to be unchecked) 


     - Wave Properties - 

Select your Wave normal maps here. You can choose to animate this maps using World Coordinates or using Uv coordinates (when "Use World Coordinates" is unchecked).
You can modify the intensity of each normal maps, the speed of the animation and the rotation angles.

The refraction is only visible if the transparency > 0. Use the refraction to distort the real reflection.

Enabling horizon let you break the tiling repetition of the normal map. You can visualize the effect by checking the "Visualize Horizon" checkbox. This option use the Normal wave1 in order to work, 
so you will need to select this map before using the Horizon properties. The tiling Multiplicator, Intensity and Speed Multiplicator options are linked to the normal wave1 and its properties.

Camera Distance : Move the waves normals away from the camera. Use a low value for the falloff to nicely blend the foreground waves and the horizon.


     - Turbulence Properties - 

The turbulence can add a chaotic look to the wave animation and a false impression of 3D waves. Use a grayscale map like the cloud texture included to create this effect. Control the intensity of the distortion 
amount (+ multiplicator if needed). Use the smooth deformation option to smooth the whole thing.

Enable Visualize to see the texture. Make sure that this options is always unchecked if not needed.


     - Foam Properties - 

Automatic edge detection! 

Control the shore foam. Modify the intensity, the rotation angles and the animation speed of the foam texture. You can also change the spread (Foam Amount) and contrast of the chosen texture. 
Select in "Heightmap Wave details" a texture to create foam everywhere else on the model. If this slot is empty the foam will be visible only on edges. 

Enable Foam Camera Distance to avoid any texture repetition.    


     - Transparency Properties - 

Control the general or only the shore transparency. The transparency can also be controlled by the camera distance. 






Real reflection shaders
_______________________


How to use
1. Create a material and assign a real reflection water shader.
2. Add the material to your model.
3. Add the Mirror Reflection script to the model too.
4. If you use Deferred rendering path you will need to enable "Invert Real Reflection" in the reflection properties. (In forward rendering Path the reflection can only be seen in the game view.... 
"Invert Real Reflection" need to be unchecked)
5. The rendering path on your camera(s) can also be turned to Forward or Deferred rendering path.


Few things to know
1. The reflection work only on planar models  
2. In the script properties you can :
- Flip the Y axis of the reflection
- Adjust the reflection depth
- Change the reflection resolution
- Choose by layers what objects are reflected or not
3. You can only see the real final reflection in Game mode for the forward rendering path. For some reason the reflections are inverted between scene view and game mode.

Tips : In forward you can check the "Invert Real Reflection" and see the final reflection in the scene view but don't forget to uncheck this option when your setup is finished.


Important : The reflection use the Y axis of your object. So Before importing a model in Unity, make sure that the local Y axis is turned in the same
direction as the reflection (cf Demo Scene).





_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _

If you are happy with the package please consider add a review or simply rate the package. This will help a lot.
Thank you.


Contact :

If you have any questions, please feel free to contact us : contact@ciconia-studio.com

More news on our work? Follow us on Twitter : twitter.com/CiconiaStudio