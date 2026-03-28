// src/data/floorplans/jurnalism/etaj7.ts
import etaj7Img from "../../../assets/floorplans/jurnalism/etaj7_jurnalism.png";
import type { FloorPlanConfig } from "../type";

export const JURNALISM_ETAJ7: FloorPlanConfig = {
  facultyKey: "jurnalism",
  floorLabel: "Jurnalism – Etaj 7",
  image: etaj7Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Laboratoare, Seminarii și Studio – Etaj 7
    { roomName: "LabMedia201", label: "LabMedia201", x: 274, y: 252 },
    { roomName: "LabMedia202", label: "LabMedia202", x: 429, y: 252 },
    { roomName: "SemPR203", label: "SemPR203", x: 600, y: 252 },
    { roomName: "StudioJ2", label: "StudioJ2", x: 760, y: 252 },
  ],
};
