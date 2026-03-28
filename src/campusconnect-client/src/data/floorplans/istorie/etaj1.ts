// src/data/floorplans/istorie/etaj1.ts
import etaj1Img from "../../../assets/floorplans/istorie/etaj1_istorie.png";
import type { FloorPlanConfig } from "../type";

export const IST_ETAJ1: FloorPlanConfig = {
  facultyKey: "istorie",
  floorLabel: "Istorie – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "Ist101", label: "Ist101", x: 300, y: 285 },
    { roomName: "Ist102", label: "Ist102", x: 472, y: 285 },
    { roomName: "Ist103", label: "Ist103", x: 656, y: 285 },
    { roomName: "Ist104", label: "Ist104", x: 103, y: 368 },
    { roomName: "Ist105", label: "Ist105", x: 840, y: 285 },
  ],
};
