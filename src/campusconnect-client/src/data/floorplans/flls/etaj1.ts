// src/data/floorplans/flls/etaj1.ts
import etaj1Img from "../../../assets/floorplans/flls/etaj1_flls.png";
import type { FloorPlanConfig } from "../type";

export const FLLS_ETAJ1: FloorPlanConfig = {
  facultyKey: "flls",
  floorLabel: "FLLS – Etaj 1",
  image: etaj1Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli – Etaj 1
    { roomName: "LLS101", label: "LLS101", x: 300, y: 295 },
    { roomName: "LLS102", label: "LLS102", x: 437, y: 295 },
    { roomName: "LLS103", label: "LLS103", x: 585, y: 295 },
    { roomName: "LLS104", label: "LLS104", x: 725, y: 295 },
    { roomName: "LLS105", label: "LLS105", x: 860, y: 295 },
  ],
};
