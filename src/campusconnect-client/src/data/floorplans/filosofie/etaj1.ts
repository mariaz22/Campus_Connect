// src/data/floorplans/filosofie/etaj1.ts
import etaj1Img from "../../../assets/floorplans/filosofie/etaj1_filo.png";
import type { FloorPlanConfig } from "../type";

export const FILO_ETAJ1: FloorPlanConfig = {
  facultyKey: "filosofie",
  floorLabel: "Filosofie – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "Filo101", label: "Filo105", x: 870, y: 250 },
    { roomName: "Filo102", label: "Filo101", x: 300, y: 250 },
    { roomName: "Filo103", label: "Filo102", x: 450, y: 250 },
    { roomName: "Filo104", label: "Filo103", x: 600, y: 250 },
    { roomName: "Filo105", label: "Filo104", x: 750, y: 250 },
  ],
};
