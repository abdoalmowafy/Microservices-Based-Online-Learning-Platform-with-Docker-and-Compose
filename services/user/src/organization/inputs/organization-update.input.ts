import { Field, InputType } from "@nestjs/graphql";
import { IsOptional, IsString, IsUrl, MinLength } from "class-validator";

@InputType()
export class OrganizationUpdateInput {
    @Field(() => String, { nullable: true })
    @IsString()
    @MinLength(2)
    @IsOptional()
    description?: string;

    @Field(() => String, { nullable: true })
    @IsUrl()
    @IsOptional()
    websiteUrl?: string;
}
