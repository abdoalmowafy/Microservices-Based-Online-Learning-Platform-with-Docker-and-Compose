-- CreateIndex
CREATE INDEX "Organization_id_ownerId_idx" ON "public"."Organization"("id", "ownerId");

-- CreateIndex
CREATE INDEX "Organization_name_idx" ON "public"."Organization"("name");
