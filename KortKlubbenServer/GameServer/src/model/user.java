package model;

import game.Card;

public class User {
    private String name;
    private String ip;

    /**
     *
     * @return name of {@link User}
     */
    public String getName(){
        return name;
    }

    /**
     *
     * @return ip of {@link User}
     */
    public String getIP(){
        return ip;
    }

    /**
     * Used To Initiate An Object Of Type {@link Card}
     * @param name
     * @param ip
     */
    public User(String name, String ip){
        this.name = name;
        this.ip = ip;
    }
}
