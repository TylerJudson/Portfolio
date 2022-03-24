

from typing import Tuple
from Button import Button
from Style import Style

from Surface import Surface
from Text import Text
import pygame
from pygame.locals import *

from colors import *

class Alert:

    def __init__(self, surface: Surface, text: Text, type: str="Simple"):
        """Initializes Alert

        Args:
            pos (Tuple[int, int]): The position of the alert on the screen
            surface (Surface): The surface of the alert used to display
            text (Text): The text that will be displayed on the alert
            type (str, optional): The type of the alert: "Simple", "Success", "Danger", "Warning". Defaults to "Simple".
        """

        self.surface = surface
        """The surface of the alert on the screen"""

        self.text = text
        """The text that will be displayed on the alert"""

        self.type = type
        """The type of the alert: "Simple", "Success", "Danger", "Warning" """

        self.color = (235, 235, 235)
        """The color of the alert"""
        self.secondaryColor = (155, 155, 155)
        """The secondary color of the alert"""
        if (self.type == "Success"):
            self.color = (147, 250, 163)
            self.secondaryColor = (47, 150, 63)
        elif (self.type == "Danger"):
            self.color = (247, 167, 163)
            self.secondaryColor = (197, 47, 43)
        elif (self.type == "Warning"):
            self.color = (255, 198, 122)
            self.secondaryColor = (175, 108, 22)

        self.closeButton = Button(Surface((self.surface.width * 9 / 10 - 20, self.surface.height / 2 - 20), (40, 40), self.color),
                                 Style(Text((20, 15), "HelveticaNeueBold.ttf", 40, "x", self.secondaryColor), borderColor=self.secondaryColor, borderRadius=5),
                                 
                                  hoverStyle=Style(Text((20, 14), "HelveticaNeueBold.ttf", 44, "x", self.color), fillColor=self.secondaryColor, borderColor=self.color, borderRadius=5))

        self.rect = Rect(0, 0, self.surface.size[0], self.surface.size[1])
        """The rect of the alert used for rendering"""

    def render(self, mousePos: Tuple[int, int]=(-1, -1)):
        """Renders the alert

        Args:
            mousePos (Tuple[int, int], optional): Where the mouse is on the window. Defaults to (-1, -1).
        """

        # clear the surface
        self.surface.clear()

        pygame.draw.rect(self.surface.display, self.color, self.rect, 0, 10)
        pygame.draw.rect(self.surface.display, self.secondaryColor, (0, 0, 10, self.surface.height), border_top_left_radius=10, border_bottom_left_radius=10)

        self.closeButton.render((mousePos[0] - self.surface.pos[0], mousePos[1] - self.surface.pos[1]))
        self.surface.display.blit(self.closeButton.surface.display, self.closeButton.surface.pos)

        self.surface.display.blit(self.text.display, self.text.rect)

    def mouseClickClose(self, mousePos: Tuple[int, int]):
        """Check to see if the mosue hit the close button

        Args:
            mousePos (Tuple[int, int]): The location of the mouse on the screen
        """

        if (self.closeButton.mouseIsHovering((mousePos[0] - self.surface.pos[0], mousePos[1] - self.surface.pos[1]))):
            return True
        return False
        

        
        


