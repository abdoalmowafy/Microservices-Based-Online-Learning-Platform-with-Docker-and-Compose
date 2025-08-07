import { Field, ID, ObjectType, registerEnumType } from "@nestjs/graphql";
import { ClassroomVisibility } from "@prisma/client";
import { ClassroomCourse } from "src/classroom-course/classroom-course";
import { ClassroomUser } from "src/classroom-user/classroom-user";
import { Organization } from "src/organization/organization";

registerEnumType(ClassroomVisibility, { name: "ClassroomVisibility" });
@ObjectType()
export class Classroom {
    @Field(() => ID)
    id: string;

    @Field(() => String)
    name: string;

    @Field(() => String, { nullable: true })
    description?: string;

    @Field(() => ClassroomVisibility)
    visibility: ClassroomVisibility;

    @Field(() => Date)
    createdAt: Date;

    @Field(() => Organization)
    organization: Organization;

    @Field(() => [ClassroomUser])
    participants: ClassroomUser[];

    @Field(() => [ClassroomCourse])
    teachingCourses: ClassroomCourse[];
}   
