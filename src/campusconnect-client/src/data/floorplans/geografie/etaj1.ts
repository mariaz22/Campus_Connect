// src/data/floorplans/geografie/etaj1.ts
import etaj1Img from "../../../assets/floorplans/geografie/etaj1_geografie.png";
import type { FloorPlanConfig } from "../type";

export const GEO_ETAJ1: FloorPlanConfig = {
  facultyKey: "geografie",
  floorLabel: "Geografie – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "Geo101", label: "Geo105", x: 856, y: 250 },
    { roomName: "Geo102", label: "Geo101", x: 300, y: 250 },
    { roomName: "Geo103", label: "Geo102", x: 450, y: 250 },
    { roomName: "Geo104", label: "Geo103", x: 600, y: 250 },
    { roomName: "Geo105", label: "Geo104", x: 750, y: 250 },
  ],
};
