-- =====================================================
-- BimOps 단지 DB 스키마 (1단지 = 1파일)
-- =====================================================

CREATE TABLE IF NOT EXISTS project (
    code            TEXT PRIMARY KEY,
    name            TEXT NOT NULL,
    building_count  INTEGER NOT NULL DEFAULT 0,
    unit_count      INTEGER NOT NULL DEFAULT 0,
    unit_types      TEXT,
    status          TEXT NOT NULL DEFAULT 'InProgress',
    last_modified   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS finish_category (
    code      TEXT PRIMARY KEY,
    name      TEXT NOT NULL,
    uom       TEXT NOT NULL,
    remark    TEXT
);

CREATE TABLE IF NOT EXISTS unit_type (
    code       TEXT PRIMARY KEY,
    name       TEXT NOT NULL,
    net_area   REAL NOT NULL DEFAULT 0,
    remark     TEXT
);

CREATE TABLE IF NOT EXISTS base_quantity (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    unit_type_code  TEXT NOT NULL,
    finish_code     TEXT NOT NULL,
    applied_room    TEXT,
    quantity        REAL NOT NULL DEFAULT 0,
    uom             TEXT NOT NULL,
    source          TEXT NOT NULL DEFAULT '수동',
    FOREIGN KEY (unit_type_code) REFERENCES unit_type(code) ON DELETE CASCADE,
    FOREIGN KEY (finish_code)    REFERENCES finish_category(code)
);

CREATE INDEX IF NOT EXISTS idx_bq_unit ON base_quantity(unit_type_code);

CREATE TABLE IF NOT EXISTS option_item (
    code           TEXT PRIMARY KEY,
    name           TEXT NOT NULL,
    category       TEXT,
    install_rooms  TEXT
);

CREATE TABLE IF NOT EXISTS option_lookup (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    option_code     TEXT NOT NULL,
    install_room    TEXT NOT NULL,
    unit_type_code  TEXT NOT NULL,
    finish_code     TEXT NOT NULL,
    delta_qty       REAL NOT NULL DEFAULT 0,
    uom             TEXT NOT NULL,
    FOREIGN KEY (option_code)    REFERENCES option_item(code) ON DELETE CASCADE,
    FOREIGN KEY (unit_type_code) REFERENCES unit_type(code),
    FOREIGN KEY (finish_code)    REFERENCES finish_category(code)
);

CREATE INDEX IF NOT EXISTS idx_lookup_option ON option_lookup(option_code);
CREATE INDEX IF NOT EXISTS idx_lookup_combo  ON option_lookup(option_code, install_room, unit_type_code, finish_code);

CREATE TABLE IF NOT EXISTS revision (
    id                INTEGER PRIMARY KEY AUTOINCREMENT,
    seq_no            INTEGER NOT NULL,
    name              TEXT NOT NULL,
    base_date         TEXT NOT NULL,
    status            TEXT NOT NULL DEFAULT 'DRAFT',  -- DRAFT / FROZEN
    source_file_path  TEXT,
    imported_at       TEXT,
    imported_by       TEXT
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_revision_name ON revision(name);

CREATE TABLE IF NOT EXISTS unit_option_selection (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    revision_id     INTEGER NOT NULL,
    building_no     TEXT NOT NULL,
    unit_no         TEXT NOT NULL,
    unit_type_code  TEXT NOT NULL,
    option_code     TEXT NOT NULL,
    install_room    TEXT,
    qty             INTEGER NOT NULL DEFAULT 1,
    selected        INTEGER NOT NULL DEFAULT 1,  -- bool: 0/1
    remark          TEXT,
    FOREIGN KEY (revision_id) REFERENCES revision(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_uos_rev  ON unit_option_selection(revision_id);
CREATE INDEX IF NOT EXISTS idx_uos_unit ON unit_option_selection(revision_id, building_no, unit_no);

CREATE TABLE IF NOT EXISTS quantity_result (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    revision_id      INTEGER NOT NULL,
    building_no      TEXT NOT NULL,
    unit_no          TEXT NOT NULL,
    finish_code      TEXT NOT NULL,
    base_qty         REAL NOT NULL DEFAULT 0,
    delta_qty        REAL NOT NULL DEFAULT 0,
    final_qty        REAL NOT NULL DEFAULT 0,
    uom              TEXT NOT NULL,
    FOREIGN KEY (revision_id) REFERENCES revision(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_qr_rev   ON quantity_result(revision_id);
CREATE INDEX IF NOT EXISTS idx_qr_unit  ON quantity_result(revision_id, building_no, unit_no);