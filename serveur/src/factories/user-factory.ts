import { UserModel } from "../models/User";
import { UserAttributes } from "../models/user-attributes";

export abstract class UserFactory {
    /**
     * Converts a UserModel (from DB) to a UserAttributes (understood by our services)
     * @param {UserModel} user
     * @returns {UserAttributes}
     */
    public static build(user: UserModel): UserAttributes {
        return {
            id: user.id,
            username: user.username,
            url: `https://example.com/users/${user.id}`,
            avatar_url: `https://example.com/users/${user.id}/avatar.jpg`,
        };
    }
}
