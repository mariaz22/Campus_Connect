// src/data/floorplans/sociologie/etaj1.ts
import etaj1Img from "../../../assets/floorplans/sociologie/etajul1_sociologie.png";
import type { FloorPlanConfig } from "../type";

export const SAS_ETAJ1: FloorPlanConfig = {
  facultyKey: "sociologie",
  floorLabel: "Sociologie – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "SAS101", label: "SAS105", x: 848, y: 250 },
    { roomName: "SAS102", label: "SAS101", x: 300, y: 250 },
    { roomName: "SAS103", label: "SAS102", x: 450, y: 250 },
    { roomName: "SAS104", label: "SAS103", x: 600, y: 250 },
    { roomName: "SAS105", label: "SAS104", x: 750, y: 250 },
  ],
};
