# Procedural Maze Generation in Unity

## 📖 Overview
This repository contains my implementation and extensions of **procedural maze & dungeon generation in Unity**, inspired by the *Procedural Maze Generation* course on Udemy by Penny de Byl and Mike.  

The project combines **maze generation algorithms**, **pathfinding (A\*)**, and **modular asset design** to create infinite maze and dungeon layouts with multiple stories and various functionalities.  
While following the course content, I also added my own modifications and experiments to implement the project in my own way.  
This repository is kept for archival purposes.  
Refer to the following link for the actual content of the course:

www.udemy.com/course/procedural-maze-dungeon-generation/

---
## 🎯 What This Project Includes
- **Maze generation algorithms**: Random crawlers, Wilson’s algorithm, Prim’s algorithm, recursive algorithms and variations that generate mazes, including handling of possible failure outcomes.  
- **Procedural corridor layouts**: Modular corridor features, snapping pieces together with geometry and math, multi-storey levels connected with manholes.  
- **Procedural dungeon layouts**: Modular dungeon features with room placement, snapping pieces together with geometry and math, multi-storey dungeons with ladder layouts managed by a Dungeon Manager, object placement system, adjustable levels, and scaling options.  
- **Pathfinding**: A\* search implementation using `DungeonPathMarker`, actual implementation to track locations and practical solutions for various challenges encountered during the course.  
- **Clairvoyance**: Inspired by Skyrim’s "Clairvoyance" spell, the pathfinding algorithm is extended to visually show the path to a desired location.  

---
## 🔗 Next Steps
- Extend object placement with weighted probabilities.  
- Add a spawn module.  
