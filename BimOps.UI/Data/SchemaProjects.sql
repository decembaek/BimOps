CREATE TABLE IF NOT EXISTS project_list (
    code            TEXT PRIMARY KEY,
    name            TEXT NOT NULL,
    building_count  INTEGER NOT NULL DEFAULT 0,
    unit_count      INTEGER NOT NULL DEFAULT 0,
    unit_types      TEXT,
    latest_round    TEXT,
    latest_status   TEXT,
    status          TEXT NOT NULL DEFAULT 'InProgress',
    last_modified   TEXT NOT NULL,
    pinned          INTEGER NOT NULL DEFAULT 0
);