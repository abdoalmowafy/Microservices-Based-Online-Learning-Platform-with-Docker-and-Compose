import { Field, ID, ObjectType, registerEnumType } from "@nestjs/graphql";
import { Classroom } from "src/classroom/classroom";
import { User } from "src/user/user";
import { RoleWithinClassroom } from "@prisma/client";

registerEnumType(RoleWithinClassroom, { name: "RoleWithinClassroom" });
@ObjectType()
export class ClassroomUser {
    @Field(() => Classroom)
    classroom: Classroom;

    @Field(() => User)
    user: User;

    @Field(() => Date)
    joinedAt: Date;

    @Field(() => Date, { nullable: true })
    leftAt?: Date;

    @Field(() => RoleWithinClassroom)
    roleWithinClassroom: RoleWithinClassroom;
}
