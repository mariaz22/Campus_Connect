// src/data/floorplans/litere/etaj1.ts
import etaj1Img from "../../../assets/floorplans/litere/etajul1_litere.png";
import type { FloorPlanConfig } from "../type";

export const LIT_ETAJ1: FloorPlanConfig = {
  facultyKey: "litere",
  floorLabel: "Litere – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "Lit101", label: "Lit105", x: 863, y: 250 },
    { roomName: "Lit102", label: "Lit101", x: 300, y: 250 },
    { roomName: "Lit103", label: "Lit102", x: 450, y: 250 },
    { roomName: "Lit104", label: "Lit103", x: 600, y: 250 },
    { roomName: "Lit105", label: "Lit104", x: 750, y: 250 },
  ],
};
