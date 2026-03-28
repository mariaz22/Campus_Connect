// src/data/floorplans/fsp/etaj1.ts
import etaj1Img from "../../../assets/floorplans/fsp/etajul1_fsp.png";
import type { FloorPlanConfig } from "../type";

export const FSP_ETAJ1: FloorPlanConfig = {
  facultyKey: "fsp",
  floorLabel: "Științe Politice – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "FSP101", label: "FSP101", x: 234, y: 246 },
    { roomName: "FSP102", label: "FSP102", x: 373, y: 246 },
    { roomName: "FSP103", label: "FSP103", x: 522, y: 246 },
    { roomName: "FSP104", label: "FSP104", x: 663, y: 246 },
    { roomName: "FSP105", label: "FSP105", x: 795, y: 246 },
  ],
};
