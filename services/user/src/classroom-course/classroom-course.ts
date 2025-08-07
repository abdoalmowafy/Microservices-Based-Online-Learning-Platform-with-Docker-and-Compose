import { Field, ID, ObjectType } from "@nestjs/graphql";
import { Classroom } from "src/classroom/classroom";
import { User } from "src/user/user";

@ObjectType()
export class ClassroomCourse {
    @Field(() => Classroom)
    classroom: Classroom;

    @Field(() => ID)
    courseId: string;

    @Field(() => User)
    addedByTeacher: User;

    @Field(() => Date)
    addedAt: Date;

    @Field(() => Date)
    removedAt: Date;
}
