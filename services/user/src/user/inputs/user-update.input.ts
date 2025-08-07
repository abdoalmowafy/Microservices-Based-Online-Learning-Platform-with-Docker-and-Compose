import { Field, InputType } from "@nestjs/graphql";
import { Gender } from "@prisma/client";
import { Type } from "class-transformer";
import { IsDate, IsEnum, IsOptional, IsString, Length, MinLength } from "class-validator";

@InputType()
export class UserUpdateInput {
    @Field(() => String, { nullable: true })
    @IsString()
    @Length(2, 20)
    @IsOptional()
    firstName?: string;

    @Field(() => String, { nullable: true })
    @IsString()
    @Length(2, 20)
    @IsOptional()
    lastName?: string;

    @Field(() => Date, { nullable: true })
    @Type(() => Date)
    @IsDate()
    @IsOptional()
    dob?: Date;

    @Field(() => Gender, { nullable: true })
    @IsEnum(Gender)
    @IsOptional()
    gender?: Gender;

    @Field(() => String, { nullable: true })
    @IsString()
    @Length(2, 2)
    @IsOptional()
    prefferedLanguage?: string;
}
