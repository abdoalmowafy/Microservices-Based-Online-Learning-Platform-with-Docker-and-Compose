import { Field, InputType } from "@nestjs/graphql";
import { RoleWithinClassroom } from "@prisma/client";
import { IsEnum, IsUUID } from "class-validator";
@InputType()
export class ClassroomUserCreateInput {
    @Field(() => String)
    @IsUUID(4)
    classroomId: string;

    @Field(() => String)
    @IsUUID(7)
    userId: string;

    @Field(() => RoleWithinClassroom)
    @IsEnum(RoleWithinClassroom)
    roleWithinClassroom: RoleWithinClassroom;
}
