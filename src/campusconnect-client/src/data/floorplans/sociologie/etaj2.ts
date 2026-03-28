// src/data/floorplans/sociologie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/sociologie/etajul2_sociologie.png";
import type { FloorPlanConfig } from "../type";

export const SAS_ETAJ2: FloorPlanConfig = {
  facultyKey: "sociologie",
  floorLabel: "Sociologie – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Seminarii și Laborator – Etaj 2
    { roomName: "SemSAS201", label: "SemSAS201", x: 278, y: 225 },
    { roomName: "SemSAS202", label: "SemSAS202", x: 456, y: 225 },
    { roomName: "SemSAS203", label: "SemSAS203", x: 633, y: 225 },
    { roomName: "LabSAS2", label: "LabSAS2", x: 826, y: 225 },
  ],
};
