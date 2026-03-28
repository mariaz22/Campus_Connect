// src/data/floorplans/fizica/etaj1.ts
import etaj1Img from "../../../assets/floorplans/fizica/etaj1_fizica.png";
import type { FloorPlanConfig } from "../type";

export const FIZ_ETAJ1: FloorPlanConfig = {
  facultyKey: "fizica",
  floorLabel: "Fizică – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "Fiz101", label: "Fiz105", x: 103, y: 314 },
    { roomName: "Fiz102", label: "Fiz101", x: 300, y: 250 },
    { roomName: "Fiz103", label: "Fiz102", x: 450, y: 250 },
    { roomName: "Fiz104", label: "Fiz103", x: 658, y: 250 },
    { roomName: "Fiz105", label: "Fiz104", x: 843, y: 250 },
  ],
};
