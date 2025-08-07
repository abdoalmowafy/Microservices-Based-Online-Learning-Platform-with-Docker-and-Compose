import { Field, InputType } from "@nestjs/graphql";
import { IsUUID } from "class-validator";

@InputType()
export class ClassroomCourseCreateInput {
    @Field(() => String)
    @IsUUID(4)
    classroomId: string;

    @Field(() => String)
    @IsUUID()
    courseId: string;

    @Field(() => String)
    @IsUUID(7)
    addedByTeacherId: string;
}