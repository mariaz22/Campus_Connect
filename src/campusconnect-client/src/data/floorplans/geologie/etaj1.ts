// src/data/floorplans/geologie/etaj1.ts
import etaj1Img from "../../../assets/floorplans/geologie/etaj1_geologie.png";
import type { FloorPlanConfig } from "../type";

export const GG_ETAJ1: FloorPlanConfig = {
  facultyKey: "geologie",
  floorLabel: "Geologie și Geofizică – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "GG101", label: "GG105", x: 100, y: 377 },
    { roomName: "GG102", label: "GG101", x: 325, y: 300 },
    { roomName: "GG103", label: "GG102", x: 525, y: 300 },
    { roomName: "GG104", label: "GG103", x: 697, y: 300 },
    { roomName: "GG105", label: "GG104", x: 855, y: 300 },
  ],
};
