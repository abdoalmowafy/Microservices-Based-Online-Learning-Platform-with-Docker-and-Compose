import { Field, InputType } from "@nestjs/graphql";
import { RoleWithinClassroom } from "@prisma/client";
import { IsEnum } from "class-validator";

@InputType()
export class ClassroomUserUpdateInput {
    @Field(() => RoleWithinClassroom, { nullable: true })
    @IsEnum(RoleWithinClassroom)
    roleWithinClassroom: RoleWithinClassroom;
}