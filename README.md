# lineBasedCave_Simplex
A variation of procedural cave generation based on line data for the cave paths and using Simplex noise for generating volume data. This project is also for my bachelor thesis I'm working on right now

LOGS:\n
16/04/2020
- First commit
- Working basic pipe volume generation. Algorithm due to change
- Added OpenSimplex Noise in Noise.compute

20/04/2020
- Auto-update mesh added
- Fixed pipe volume generation algorithm. Issue still occur at 0th line
- Added option for world-space noise input coordinate. Input coordinate will be automatically changed to line's local coordinate if disabled
- Planned further optimization for auto update

01/05/2020
- Stable cave generation
- Fixed 0th line issue on pipe volume generation algoritm
- Multiple chunks feature added to help increase the size of generated cave environment while overcoming Unity's 16-bit mesh index format

KNOWN BUG:
- Due to GarbageCollector behaviour to prevent memory leak, _pointsBuffer are deleted. Thus, the mesh is not generated correctly.
- I also found that the number of DispatchThread are also causing the same effect as before. This proof is tested by first keep the Lines configuration and knowing the value of numVoxel and size at Voxel.cs at which defection is detected, then I doubled the size but half the numVoxel. No defection is detected afterwards.

SOLUTIONS:
- Density generation process is now done in loop. The loop runs by the total number of Line objects. This solves bug no. 1.
- Find at which DispatchThread value a compute shader can handle at maximum or make the size of DispatchThread as (8,8,8), but divide the voxel with each division's size is (8,8,8). This proposed solution is yet to be tested for bug no. 2.