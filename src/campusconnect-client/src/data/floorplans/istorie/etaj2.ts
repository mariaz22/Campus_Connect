// src/data/floorplans/istorie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/istorie/etaj2_istorie.png";
import type { FloorPlanConfig } from "../type";

export const IST_ETAJ2: FloorPlanConfig = {
  facultyKey: "istorie",
  floorLabel: "Istorie – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Seminarii – Etaj 2
    { roomName: "SemIst201", label: "SemIst201", x: 414, y: 304 },
    { roomName: "SemIst202", label: "SemIst202", x: 586, y: 304 },
    { roomName: "SemIst203", label: "SemIst203", x: 750, y: 304 },
  ],
};
