// src/data/floorplans/fsp/etaj2.ts
import etaj2Img from "../../../assets/floorplans/fsp/etajul2_fsp.png";
import type { FloorPlanConfig } from "../type";

export const FSP_ETAJ2: FloorPlanConfig = {
  facultyKey: "fsp",
  floorLabel: "Științe Politice – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Seminarii și Laborator – Etaj 2
    { roomName: "SemFSP201", label: "SemFSP201", x: 217, y: 317 },
    { roomName: "SemFSP202", label: "SemFSP202", x: 365, y: 317 },
    { roomName: "SemFSP203", label: "SemFSP203", x: 499, y: 317 },
    { roomName: "LabFSP2", label: "LabFSP2", x: 639, y: 317 },
  ],
};
