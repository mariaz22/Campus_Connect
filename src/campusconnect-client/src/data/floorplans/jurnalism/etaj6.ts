// src/data/floorplans/jurnalism/etaj6.ts
import etaj6Img from "../../../assets/floorplans/jurnalism/etaj6_jurnalism.png";
import type { FloorPlanConfig } from "../type";

export const JURNALISM_ETAJ6: FloorPlanConfig = {
  facultyKey: "jurnalism",
  floorLabel: "Jurnalism – Etaj 6",
  image: etaj6Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Săli și Amfiteatru – Etaj 6
    { roomName: "J101", label: "J101", x: 259, y: 317 },
    { roomName: "J102", label: "J102", x: 370, y: 317 },
    { roomName: "J103", label: "J103", x: 477, y: 317 },
    { roomName: "J104", label: "J104", x: 590, y: 317 },
    { roomName: "J105", label: "J105", x: 106, y: 341 },
    { roomName: "AmfJ1", label: "AmfJ1", x: 780, y: 317 },
  ],
};
