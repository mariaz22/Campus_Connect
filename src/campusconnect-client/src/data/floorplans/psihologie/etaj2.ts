// src/data/floorplans/psihologie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/psihologie/etaj2_psihologie.png";
import type { FloorPlanConfig } from "../type";

export const PSI_ETAJ2: FloorPlanConfig = {
  facultyKey: "psihologie",
  floorLabel: "Psihologie – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Laboratoare și Seminarii – Etaj 2
    { roomName: "LabPsi201", label: "LabPsi201", x: 303, y: 212 },
    { roomName: "LabPsi202", label: "LabPsi202", x: 553, y: 212 },
    { roomName: "SemEdu203", label: "SemEdu203", x: 797, y: 212 },
  ],
};
