// src/data/floorplans/fizica/etaj2.ts
import etaj2Img from "../../../assets/floorplans/fizica/etaj2_fizica.png";
import type { FloorPlanConfig } from "../type";

export const FIZ_ETAJ2: FloorPlanConfig = {
  facultyKey: "fizica",
  floorLabel: "Fizică – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Laboratoare – Etaj 2
    { roomName: "LabFiz201", label: "LabFiz201", x: 322, y: 306 },
    { roomName: "LabFiz202", label: "LabFiz202", x: 500, y: 306 },
    { roomName: "LabFiz203", label: "LabFiz203", x: 838, y: 306 },
  ],
};
