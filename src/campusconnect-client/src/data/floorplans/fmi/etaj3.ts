// src/data/floorplans/fmi/etaj3.ts
import etaj3Img from "../../../assets/floorplans/fmi/etaj3.png";

// IMPORTANT: importăm tipurile ca să fie 100% compatibile cu FloorPlanViewer
import type { FloorPlanConfig } from "./parter";

export const FMI_ETAJ3: FloorPlanConfig = {
  facultyKey: "fmi",
  floorLabel: "FMI – Etaj 3",
  image: etaj3Img,

  // ✅ exact dimensiunea reală a imaginii (din poza ta)
  canvas: { w: 517, h: 467 },

  pins: [
    // ✅ DB name MUST match exact:
    // "Amf. Gheorghe Țițeica"
    //
    // COORDONATE: sunt un start bun (aprox). Le ajustezi ușor după ce vezi pin-ul în UI.
    { roomName: "Amf. Gheorghe Țițeica", label: "Amf. Țițeica", x: 112, y: 256 },
  ],
};
