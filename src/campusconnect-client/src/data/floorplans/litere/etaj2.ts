// src/data/floorplans/litere/etaj2.ts
import etaj2Img from "../../../assets/floorplans/litere/etajul2_litere.png";
import type { FloorPlanConfig } from "../type";

export const LIT_ETAJ2: FloorPlanConfig = {
  facultyKey: "litere",
  floorLabel: "Litere – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Seminarii – Etaj 2
    { roomName: "SemLit201", label: "SemLit201", x: 323, y: 212 },
    { roomName: "SemLit202", label: "SemLit202", x: 558, y: 212 },
    { roomName: "SemLit203", label: "SemLit203", x: 803, y: 212 },
  ],
};
