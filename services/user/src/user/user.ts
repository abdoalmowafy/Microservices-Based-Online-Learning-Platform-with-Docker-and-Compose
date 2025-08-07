import { Field, ID, ObjectType, registerEnumType } from "@nestjs/graphql";
import { ClassroomCourse } from "src/classroom-course/classroom-course";
import { ClassroomUser } from "src/classroom-user/classroom-user";
import { Organization } from "src/organization/organization";
import { Gender } from "@prisma/client";

registerEnumType(Gender, { name: "Gender" });
@ObjectType()
export class User {
    @Field(() => ID)
    id: string;

    @Field(() => String)
    email: string;

    @Field(() => String, { nullable: true })
    firstName?: string;

    @Field(() => String, { nullable: true })
    lastName?: string;

    @Field(() => Date, { nullable: true })
    dob?: Date;

    @Field(() => Gender, { nullable: true })
    gender?: Gender;

    @Field(() => String, { nullable: true })
    avatarUrl?: string;

    @Field(() => String, { nullable: true })
    prefferedLanguage?: string;

    @Field(() => Date)
    createdAt: Date;

    @Field(() => [Organization])
    ownedOrgs: Organization[];

    @Field(() => [Organization])
    moderatedOrgs: Organization[];

    @Field(() => [ClassroomUser])
    classrooms: ClassroomUser[];

    @Field(() => [ClassroomCourse])
    addedCoursesToClassroomsAsTeacher: ClassroomCourse[];
}