import { Field, InputType } from "@nestjs/graphql";
import { ClassroomVisibility } from "@prisma/client";
import { IsEnum, IsOptional, IsString, Length } from "class-validator";

@InputType()
export class ClassroomUpdateInput {
    @Field(() => String, { nullable: true })
    @IsString()
    @Length(5, 20)
    @IsOptional()
    name?: string;

    @Field(() => String, { nullable: true })
    @IsString()
    @IsOptional()
    description?: string;

    @Field(() => ClassroomVisibility, { nullable: true })
    @IsEnum(ClassroomVisibility)
    @IsOptional()
    visibility?: ClassroomVisibility;
}