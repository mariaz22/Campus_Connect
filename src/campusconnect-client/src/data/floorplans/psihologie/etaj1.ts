// src/data/floorplans/psihologie/etaj1.ts
import etaj1Img from "../../../assets/floorplans/psihologie/etaj1_psihologie.png";
import type { FloorPlanConfig } from "../type";

export const PSI_ETAJ1: FloorPlanConfig = {
  facultyKey: "psihologie",
  floorLabel: "Psihologie – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "Psi101", label: "Psi104", x: 866, y: 250 },
    { roomName: "Psi102", label: "Psi101", x: 300, y: 250 },
    { roomName: "Psi103", label: "Psi102", x: 450, y: 250 },
    { roomName: "Psi104", label: "Psi103", x: 600, y: 250 },
    { roomName: "Psi105", label: "Psi104", x: 750, y: 250 },
  ],
};
