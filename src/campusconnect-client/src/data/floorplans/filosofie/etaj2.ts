// src/data/floorplans/filosofie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/filosofie/etaj2_filo.png";
import type { FloorPlanConfig } from "../type";

export const FILO_ETAJ2: FloorPlanConfig = {
  facultyKey: "filosofie",
  floorLabel: "Filosofie – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Seminarii – Etaj 2
    { roomName: "SemFilo201", label: "SemFilo201", x: 323, y: 301 },
    { roomName: "SemFilo202", label: "SemFilo202", x: 558, y: 301 },
    { roomName: "SemFilo203", label: "SemFilo203", x: 792, y: 301 },
  ],
};
