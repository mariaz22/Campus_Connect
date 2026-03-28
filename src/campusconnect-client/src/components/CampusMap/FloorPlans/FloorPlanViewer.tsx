// src/components/CampusMap/FloorPlans/FloorPlanViewer.tsx
import { useMemo } from "react";
import type { FloorPlanConfig, Status } from "../../../data/floorplans/type";

type SimpleRoom = {
  name: string;
  currentStatus: Status;
};

type Props = {
  config: FloorPlanConfig;
  rooms: SimpleRoom[];
  selectedRoomName?: string;
  onSelectRoomName?: (name: string) => void;
};

const statusDotClass = (status: Status) => {
  switch (status) {
    case "Free":
      return "bg-green-500";
    case "Occupied":
      return "bg-red-500";
    case "OccupiedSoon":
      return "bg-yellow-500";
    default:
      return "bg-gray-500";
  }
};

export const FloorPlanViewer = ({
  config,
  rooms,
  selectedRoomName,
  onSelectRoomName,
}: Props) => {
  const roomByName = useMemo(() => {
    const map = new Map<string, SimpleRoom>();
    rooms.forEach((r) => map.set(r.name, r));
    return map;
  }, [rooms]);

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        {/* <h3 className="font-semibold">Schiță: {config.floorLabel}</h3> */}
        {/* <span className="text-xs text-muted-foreground">
          Verde=liber, Galben=în curând, Roșu=ocupat
        </span> */}
      </div>

      <div className="w-full rounded-lg border bg-background overflow-hidden">
        <div className="relative w-full">
          <img
            src={config.image}
            alt={config.floorLabel}
            className="w-full h-auto block select-none"
            draggable={false}
          />

          {/* Overlay peste imagine (coord în procente, derivate din canvas px) */}
          <div className="absolute inset-0">
            {config.pins.map((p) => {
              const room = roomByName.get(p.roomName);
              const isSelected = selectedRoomName === p.roomName;

              const leftPct = (p.x / config.canvas.w) * 100;
              const topPct = (p.y / config.canvas.h) * 100;

              const dotClass = isSelected
                ? "bg-purple-600"
                : statusDotClass(room?.currentStatus ?? "Unknown");

              return (
                <button
                  key={p.roomName}
                  type="button"
                  onClick={(ev) => {
                    ev.stopPropagation();
                    onSelectRoomName?.(p.roomName);
                  }}
                  className={[
                    "absolute",
                    onSelectRoomName ? "cursor-pointer" : "cursor-default",
                  ].join(" ")}
                  style={{ left: `${leftPct}%`, top: `${topPct}%` }}
                  title={`${p.label ?? p.roomName}${room ? ` • ${room.currentStatus}` : ""}`}
                >
                  {/* Anchor-ul (centrul) este DOT-ul, nu dot+label */}
                  <span className="relative block -translate-x-1/2 -translate-y-1/2">
                    {isSelected && (
                      <span className="absolute inset-0 rounded-full bg-purple-500/35 animate-ping" />
                    )}

                    <span
                      className={[
                        "relative block rounded-full h-4 w-4",
                        dotClass,
                        "shadow-md",
                        isSelected
                          ? "ring-4 ring-purple-500/40"
                          : "ring-2 ring-white/80",
                      ].join(" ")}
                    />

                    {isSelected && (
                      <span className="absolute left-6 top-1/2 -translate-y-1/2 px-2 py-1 rounded-md text-xs font-semibold bg-white/95 border shadow whitespace-nowrap">
                        {p.label ?? p.roomName}
                      </span>
                    )}
                  </span>
                </button>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
};
