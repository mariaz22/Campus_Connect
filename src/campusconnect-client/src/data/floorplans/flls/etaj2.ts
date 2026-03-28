// src/data/floorplans/flls/etaj2.ts
import etaj2Img from "../../../assets/floorplans/flls/etaj2.png";
import type { FloorPlanConfig } from "../type";

export const FLLS_ETAJ2: FloorPlanConfig = {
  facultyKey: "flls",
  floorLabel: "FLLS – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Seminarii și Laborator – Etaj 2
    { roomName: "SemLLS201", label: "SemLLS201", x: 267, y: 303 },
    { roomName: "SemLLS202", label: "SemLLS202", x: 434, y: 303 },
    { roomName: "SemLLS203", label: "SemLLS203", x: 633, y: 303 },
    { roomName: "LabLLS2", label: "LabLLS2", x: 835, y: 303 },
  ],
};
