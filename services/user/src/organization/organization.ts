import { Field, ID, ObjectType } from "@nestjs/graphql";
import { Classroom } from "src/classroom/classroom";
import { User } from "src/user/user";
@ObjectType()
export class Organization {
    @Field(() => ID)
    id: string;

    @Field(() => String)
    name: string;

    @Field(() => String, { nullable: true })
    description?: string;

    @Field(() => String, { nullable: true })
    logoUrl?: string;

    @Field(() => String, { nullable: true })
    websiteUrl?: string;

    @Field(() => User)
    owner: User;

    @Field(() => Date)
    createdAt: Date;

    @Field(() => [Classroom])
    classrooms: Classroom[];

    @Field(() => [User])
    moderators: User[];
}