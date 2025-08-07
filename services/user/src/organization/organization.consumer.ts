import { RabbitSubscribe } from "@golevelup/nestjs-rabbitmq";
import { Injectable, LoggerService } from "@nestjs/common";
import { OrganizationService } from "./organization.service";
import { OrganizationLogoEvent } from "./events/organization-logo.event";

@Injectable()
export class UserConsumer {
    constructor(private readonly organizationService: OrganizationService, private readonly logger: LoggerService) { }

    @RabbitSubscribe({
        exchange: 'organization',
        routingKey: 'organization.logo',
        queue: 'organization.logo',
    })
    async organizationAvatar(payload: OrganizationLogoEvent) {
        try {
            await this.organizationService.logo(payload.id, payload.url);
        } catch (error) {
            this.logger.error(error);
        }
    }
}