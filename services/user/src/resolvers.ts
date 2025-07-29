import prisma from "./prismaClient";

export const resolvers = {
    Query: {
        users: async () => {
            return await prisma.user.findMany();
        },
    },
};
