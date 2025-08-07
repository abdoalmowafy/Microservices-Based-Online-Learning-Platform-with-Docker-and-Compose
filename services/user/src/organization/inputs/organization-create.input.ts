import { Field, InputType } from "@nestjs/graphql";
import { IsOptional, IsString, IsUrl, Length, Min, MinLength } from "class-validator";

@InputType()
export class OrganizationCreateInput {
    @Field(() => String)
    @IsString()
    @Length(5, 20)
    name: string;

    @Field(() => String, { nullable: true })
    @IsString()
    @MinLength(2)
    @IsOptional()
    description: string;

    @Field(() => String, { nullable: true })
    @IsUrl()
    @IsOptional()
    websiteUrl: string;
}
