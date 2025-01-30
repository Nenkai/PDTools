# PDTools.Files

Various classes for reading into all proprietary file formats. Refer to the modding hub for more information.

* Courses
  * Autodrive -> `.ad` files, most GTs.
  * Minimap -> `.map` files, GT5/6, maybe above.
  * PS2
    * CourseSound - For CourseSound headers within GT4 course/track files with 'CRSS' magic.. Not finished.
    * DCourseEffect - For CourseSound headers within GT4 course/track files with 'GTFX' magic. Not finished.
    * Runway - For runway files within GT4 course/track files with 'RNW4'/'4WNR' magic. Not fully completed, there may be a KDTree that needs to be reversed. A CourseV (aka meter distance calculator) is included, reversed.
    * VisionList - For vision lists within GT4 course/track files as 'Vls0' magic. Handles regions to display depending on current position, based on voronoi. Not finished.
    * MinimapSet - For minimap files within GT4 course/track files as 'GTCM' magic. Has textures and other parameters. Not finished.
  * PS3
    * CourseDataFile - Loads course packs from the `crs` folder.
  * Runway - For runway files for >=GT5 to 7. **This is a versioned file.** Same deal, KDTree (used for raycasting?) hasn't been figured out. GTS/7 uses 64-bit pointers.
  * GT4ReplayData - Reads GT4 replay data (and camera params present within course files) with 'REP4' magic. Not finished.
* Fonts - GT5/GT6 `.vec` font reader. Bit-packed and presumably sent to SPU (or shader) directly. We got lucky on this one as a regular game function that wasn't called had logic to read it. For most part done.
* Models
  * PS2
    * CarModel0 - Reads GT2K car model files.
    * CarModel1 - Reads GT3 car model files. Included is a builder.
    * ModelSet - Reads GT2K (ModelSet0), GT3 (ModelSet1) & GT4 (ModelSet2) files. GT4 is not completely done. GT3 is close.
    * RenderCommands - Render commands used by the PGL model render command interpreter for all PS2 GTs.
  * PS3
    * ModelSet3 - ModelSet3/MDL3 reader for PS3 GTs. **This format is versioned itself and may vary between PS3/PSP games.**
      * FVF - Flexible Vertex definitions/layouts.
      * PackedMesh - The dreaded 'PMSH'. This isn't figured out. This is bit-packed mesh data sent to SPU for interpreting.
      * ShapeStream - For links between meshes/shapes and data contained within `.shapestream` files.
      * NOTE: The relation between shaders and materials hasn't been made yet..
    * PGLCommands - Render commands used by the PGL model render command interpreter for all PS3 GTs (and GTPSP).
  * Shaders - Classes for shaders
  * ShapeStream - ShapeStream reader
  * VM - Virtual machine used within model sets to script certain body parts, used in most GTs since GT4. The VM instance/structure sits pre-allocated in the model set buffer itself.
* Sound - Mostly intended for GT4 formats such as midi-based `.sqt`. MusicInf is the playlist file.
* Textures
  * PS2 - PS2 TextureSet1, included is a builder. **Not fully finished, Polyphony has gone utterly mad abusing GS registers to fit textures wherever possible.** We don't save enough space to be able to edit existing models rather than scratch. GTPS2ModelTool uses this.
  * PS3 - PS3 TextureSet3 variant reader.
  * PS4 - PS4 TextureSet3 variant reader.
  * PSP - PSP TextureSet3 variant reader.

NOTE: Some variable names on the PS2 side of things were recovered thanks to asserts found in [1668.elf](https://archive.org/details/gthd-ps3-debug-binaries)
