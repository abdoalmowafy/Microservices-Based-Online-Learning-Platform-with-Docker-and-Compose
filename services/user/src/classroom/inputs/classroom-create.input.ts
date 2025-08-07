import { Field, InputType } from "@nestjs/graphql";
import { ClassroomVisibility } from "@prisma/client";
import { IsEnum, IsOptional, IsString, IsUUID, Length, MinLength } from "class-validator";

@InputType()
export class ClassroomCreateInput {
    @Field(() => String)
    @IsString()
    @Length(5, 20)
    name: string;

    @Field(() => String, { nullable: true })
    @IsString()
    @MinLength(2)
    @IsOptional()
    description?: string;

    @Field(() => ClassroomVisibility)
    @IsEnum(ClassroomVisibility)
    visibility: ClassroomVisibility;

    @Field(() => String)
    @IsUUID(4)
    organizationId: string;
}