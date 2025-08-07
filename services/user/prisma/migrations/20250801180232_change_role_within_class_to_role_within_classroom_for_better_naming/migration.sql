/*
  Warnings:

  - You are about to drop the column `roleWithinClass` on the `ClassroomUser` table. All the data in the column will be lost.
  - You are about to drop the `OrganizationModerator` table. If the table is not empty, all the data it contains will be lost.
  - Added the required column `roleWithinClassroom` to the `ClassroomUser` table without a default value. This is not possible if the table is not empty.

*/
-- CreateEnum
CREATE TYPE "public"."RoleWithinClassroom" AS ENUM ('TEACHER', 'ASSISTANT', 'STUDENT');

-- DropForeignKey
ALTER TABLE "public"."OrganizationModerator" DROP CONSTRAINT "OrganizationModerator_organizationId_fkey";

-- DropForeignKey
ALTER TABLE "public"."OrganizationModerator" DROP CONSTRAINT "OrganizationModerator_userId_fkey";

-- AlterTable
ALTER TABLE "public"."ClassroomUser" DROP COLUMN "roleWithinClass",
ADD COLUMN     "roleWithinClassroom" "public"."RoleWithinClassroom" NOT NULL;

-- DropTable
DROP TABLE "public"."OrganizationModerator";

-- DropEnum
DROP TYPE "public"."RoleWithinClass";

-- CreateTable
CREATE TABLE "public"."_OrganizationModerators" (
    "A" TEXT NOT NULL,
    "B" TEXT NOT NULL,

    CONSTRAINT "_OrganizationModerators_AB_pkey" PRIMARY KEY ("A","B")
);

-- CreateIndex
CREATE INDEX "_OrganizationModerators_B_index" ON "public"."_OrganizationModerators"("B");

-- AddForeignKey
ALTER TABLE "public"."_OrganizationModerators" ADD CONSTRAINT "_OrganizationModerators_A_fkey" FOREIGN KEY ("A") REFERENCES "public"."Organization"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "public"."_OrganizationModerators" ADD CONSTRAINT "_OrganizationModerators_B_fkey" FOREIGN KEY ("B") REFERENCES "public"."User"("id") ON DELETE CASCADE ON UPDATE CASCADE;
